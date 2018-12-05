using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using MySql.Data.MySqlClient;
using P3.Models;

namespace P3.Controllers
{
	public class LoginController : Controller
	{
		// GET: Login
		public ActionResult Index()
		{
			Login login = new Login() {LoggedIn = false};

			// Check if we're already logged in
			if (!String.IsNullOrEmpty(Session["user"] as string) && !String.IsNullOrEmpty(Session["role"] as string))
			{
				login.LoggedIn = true;
				login.Username = Session["user"].ToString();
				login.Role = Session["role"].ToString();

				return View(login);
			}

			bool isPost = Request.HttpMethod == "POST";
			if (isPost)
			{
				string constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
				using (MySqlConnection con = new MySqlConnection(constr))
				{
					try
					{
						con.Open();
						string query = "SELECT Nummer, Salt, Hash, Aktiv FROM Benutzer WHERE Nutzername = @name LIMIT 1";
						using (MySqlCommand cmd = new MySqlCommand(query, con))
						{
							cmd.Parameters.AddWithValue("name", Request["loginName"]);
							using (MySqlDataReader reader = cmd.ExecuteReader())
							{
								if (reader.Read())
								{
									if (Convert.ToInt32(reader["Aktiv"]) != 1)
									{
										throw new Exception("Benutzer noch nicht aktiviert!");
									}
									login.Username = Request["loginName"];
									login.ID = Convert.ToInt32(reader["Nummer"]);
									login.Salt = reader["Salt"].ToString();
									login.Hash = reader["Hash"].ToString();
								}
							}
						}

						if (!String.IsNullOrEmpty(login.Salt) && !String.IsNullOrEmpty(login.Hash))
						{
							if (PasswordStorage.VerifyPassword(Request["loginPassword"],
								$"sha1:64000:18:{login.Salt}:{login.Hash}"))
								login.LoggedIn = true;
						}

						if (login.LoggedIn)
						{
							using (MySqlCommand cmd = new MySqlCommand("Nutzerrolle", con))
							{
								cmd.CommandType = CommandType.StoredProcedure;
								cmd.Parameters.AddWithValue("ID", login.ID);
								MySqlParameter role = new MySqlParameter("Role", MySqlDbType.VarChar, 25)
									{Direction = ParameterDirection.Output};
								cmd.Parameters.Add(role);
								cmd.ExecuteNonQuery();
								login.Role = role.Value != DBNull.Value ? role.Value.ToString() : "null";
							}
						}
						else
						{
							throw new Exception("Das hat nicht geklappt! Bitte versuchen Sie es erneut.");
						}

						con.Close();
					}
					catch (Exception e)
					{
						con.Close();
						ModelState.AddModelError("Error", e.Message);
						login.Failed = true;
						return View(login);
					}
				}

				Session["user"] = login.Username;
				Session["role"] = login.Role;
			}

			return View(login);
		}

		public ActionResult Logout()
		{
			if (!String.IsNullOrEmpty(Session["user"] as string) || !String.IsNullOrEmpty(Session["role"] as string))
			{
				Session["user"] = null;
				Session["role"] = null;
				ModelState.AddModelError("Success", "Erfolgreich abgemeldet.");
			}
			else
				ModelState.AddModelError("Error", "Da ist etwas schief gegangen!");

			return View();
		}

		public ActionResult Register()
		{
			//Already logged in -> no need to register
			if (!String.IsNullOrEmpty(Session["user"] as string) || !String.IsNullOrEmpty(Session["role"] as string))
				return View();

			bool isPost = Request.HttpMethod == "POST";
			if (isPost)
			{
				if (Request["password"] == null || Request["password"] != Request["passwordRepeat"])
				{
					ModelState.AddModelError("Error", "Die Passwörter stimmen nicht über ein!");
					return View();
				}

				string result = PasswordStorage.CreateHash(Request["password"]);
				var results = result.Split(':');
				string hash = results.Last();
				string salt = results[results.Length - 2];

				string constr = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
				using (MySqlConnection con = new MySqlConnection(constr))
				{
					MySqlTransaction tr = null;
					try
					{
						con.Open();
						// innerhalb der Connection con eine Transaktion beginnen
						tr = con.BeginTransaction();
						using (MySqlCommand cmd = new MySqlCommand() {Connection = con, Transaction = tr})
						{
							cmd.CommandText =
								"INSERT INTO Benutzer(`E-Mail`, Nutzername, Geburtsdatum, Anlegedatum, Aktiv, Vorname, Nachname, Salt, Hash) " +
								"VALUES(@mail, @name, @date, CURDATE(), 0, @firstName, @lastName, @salt, @hash)";
							cmd.Parameters.AddWithValue("mail", Request["mail"]);
							cmd.Parameters.AddWithValue("name", Request["name"]);
							cmd.Parameters.AddWithValue("date", !String.IsNullOrEmpty(Request["birthdate"]) ? Request["birthdate"] : "null");
							cmd.Parameters.AddWithValue("firstName", Request["firstName"]);
							cmd.Parameters.AddWithValue("lastName", Request["lastName"]);
							cmd.Parameters.AddWithValue("salt", salt);
							cmd.Parameters.AddWithValue("hash", hash);
							var rows = cmd.ExecuteNonQuery();

							cmd.Parameters.Clear();
							cmd.Parameters.AddWithValue("id", cmd.LastInsertedId);
							if (Request["role"] == "Mitarbeiter" || Request["role"] == "Student")
							{
								cmd.CommandText = "INSERT INTO `FH Angehörige`(Nummer) VALUES(@id)";
								cmd.ExecuteNonQuery();
							}

							switch (Request["role"])
							{
								case "Gast":
									if (!String.IsNullOrEmpty(Request["expireDate"]))
									{
										cmd.CommandText =
											"INSERT INTO Gäste(Nummer, Ablaufdatum, Grund) VALUES(@id, @date, @reason)";
										cmd.Parameters.AddWithValue("date", Request["expireDate"]);
									}
									else
										cmd.CommandText = "INSERT INTO Gäste(Nummer, Grund) VALUES(@id, @reason)";
									cmd.Parameters.AddWithValue("reason", !String.IsNullOrEmpty(Request["reason"]) ? Request["reason"] : "null");
									break;
								case "Mitarbeiter":
									cmd.CommandText = "INSERT INTO Mitarbeiter(Nummer, Telefon, Büro) VALUES(@id, @phone, @office)";
									cmd.Parameters.AddWithValue("phone", !String.IsNullOrEmpty(Request["phone"]) ? Request["phone"] : "null");
									cmd.Parameters.AddWithValue("office", !String.IsNullOrEmpty(Request["office"]) ? Request["office"] : "null");
									break;
								case "Student":
									cmd.CommandText = "INSERT INTO Studenten(Nummer, Matrikelnummer, Studiengang) VALUES(@id, @matriculationNumber, @degree)";
									cmd.Parameters.AddWithValue("matriculationNumber", Request["matriculationNumber"]);
									cmd.Parameters.AddWithValue("degree", Request["degree"]);
									break;

							}
							rows = cmd.ExecuteNonQuery();

							tr.Commit();
						}

						con.Close();
					}
					catch (Exception e)
					{
						tr?.Rollback();
						con.Close();
						ModelState.AddModelError("Error", e.Message);
						return View();
					}

					//TODO: go to /Index or /Logout (depends if the client is logged in afterwards) after registering successfully
				}
			}

			return View();
		}

		#region PasswordStorage

		class InvalidHashException : Exception
		{
			public InvalidHashException()
			{
			}

			public InvalidHashException(string message)
				: base(message)
			{
			}

			public InvalidHashException(string message, Exception inner)
				: base(message, inner)
			{
			}
		}

		class CannotPerformOperationException : Exception
		{
			public CannotPerformOperationException()
			{
			}

			public CannotPerformOperationException(string message)
				: base(message)
			{
			}

			public CannotPerformOperationException(string message, Exception inner)
				: base(message, inner)
			{
			}
		}

		public class PasswordStorage
		{
			// These constants may be changed without breaking existing hashes.
			public const int SALT_BYTES = 24;
			public const int HASH_BYTES = 18;
			public const int PBKDF2_ITERATIONS = 64000;

			// These constants define the encoding and may not be changed.
			public const int HASH_SECTIONS = 5;
			public const int HASH_ALGORITHM_INDEX = 0;
			public const int ITERATION_INDEX = 1;
			public const int HASH_SIZE_INDEX = 2;
			public const int SALT_INDEX = 3;
			public const int PBKDF2_INDEX = 4;

			public static string CreateHash(string password)
			{
				// Generate a random salt
				byte[] salt = new byte[SALT_BYTES];
				try
				{
					using (RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider())
					{
						csprng.GetBytes(salt);
					}
				}
				catch (CryptographicException ex)
				{
					throw new CannotPerformOperationException(
						"Random number generator not available.",
						ex
					);
				}
				catch (ArgumentNullException ex)
				{
					throw new CannotPerformOperationException(
						"Invalid argument given to random number generator.",
						ex
					);
				}

				byte[] hash = PBKDF2(password, salt, PBKDF2_ITERATIONS, HASH_BYTES);

				// format: algorithm:iterations:hashSize:salt:hash
				String parts = "sha1:" +
				               PBKDF2_ITERATIONS +
				               ":" +
				               hash.Length +
				               ":" +
				               Convert.ToBase64String(salt) +
				               ":" +
				               Convert.ToBase64String(hash);
				return parts;
			}

			public static bool VerifyPassword(string password, string goodHash)
			{
				char[] delimiter = {':'};
				string[] split = goodHash.Split(delimiter);

				if (split.Length != HASH_SECTIONS)
				{
					throw new InvalidHashException(
						"Fields are missing from the password hash."
					);
				}

				// We only support SHA1 with C#.
				if (split[HASH_ALGORITHM_INDEX] != "sha1")
				{
					throw new CannotPerformOperationException(
						"Unsupported hash type."
					);
				}

				int iterations = 0;
				try
				{
					iterations = Int32.Parse(split[ITERATION_INDEX]);
				}
				catch (ArgumentNullException ex)
				{
					throw new CannotPerformOperationException(
						"Invalid argument given to Int32.Parse",
						ex
					);
				}
				catch (FormatException ex)
				{
					throw new InvalidHashException(
						"Could not parse the iteration count as an integer.",
						ex
					);
				}
				catch (OverflowException ex)
				{
					throw new InvalidHashException(
						"The iteration count is too large to be represented.",
						ex
					);
				}

				if (iterations < 1)
				{
					throw new InvalidHashException(
						"Invalid number of iterations. Must be >= 1."
					);
				}

				byte[] salt = null;
				try
				{
					salt = Convert.FromBase64String(split[SALT_INDEX]);
				}
				catch (ArgumentNullException ex)
				{
					throw new CannotPerformOperationException(
						"Invalid argument given to Convert.FromBase64String",
						ex
					);
				}
				catch (FormatException ex)
				{
					throw new InvalidHashException(
						"Base64 decoding of salt failed.",
						ex
					);
				}

				byte[] hash = null;
				try
				{
					hash = Convert.FromBase64String(split[PBKDF2_INDEX]);
				}
				catch (ArgumentNullException ex)
				{
					throw new CannotPerformOperationException(
						"Invalid argument given to Convert.FromBase64String",
						ex
					);
				}
				catch (FormatException ex)
				{
					throw new InvalidHashException(
						"Base64 decoding of pbkdf2 output failed.",
						ex
					);
				}

				int storedHashSize = 0;
				try
				{
					storedHashSize = Int32.Parse(split[HASH_SIZE_INDEX]);
				}
				catch (ArgumentNullException ex)
				{
					throw new CannotPerformOperationException(
						"Invalid argument given to Int32.Parse",
						ex
					);
				}
				catch (FormatException ex)
				{
					throw new InvalidHashException(
						"Could not parse the hash size as an integer.",
						ex
					);
				}
				catch (OverflowException ex)
				{
					throw new InvalidHashException(
						"The hash size is too large to be represented.",
						ex
					);
				}

				if (storedHashSize != hash.Length)
				{
					throw new InvalidHashException(
						"Hash length doesn't match stored hash length."
					);
				}

				byte[] testHash = PBKDF2(password, salt, iterations, hash.Length);
				return SlowEquals(hash, testHash);
			}

			private static bool SlowEquals(byte[] a, byte[] b)
			{
				uint diff = (uint) a.Length ^ (uint) b.Length;
				for (int i = 0; i < a.Length && i < b.Length; i++)
				{
					diff |= (uint) (a[i] ^ b[i]);
				}

				return diff == 0;
			}

			private static byte[] PBKDF2(string password, byte[] salt, int iterations, int outputBytes)
			{
				using (Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt))
				{
					pbkdf2.IterationCount = iterations;
					return pbkdf2.GetBytes(outputBytes);
				}
			}
		}

		#endregion
	}
}