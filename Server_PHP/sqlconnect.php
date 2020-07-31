<?php
	// Getting the DB Connection
	function getDBConnection()
	{
		$db = @mysqli_connect("localhost", "root", "") or die("Verbindung zu MySQL gescheitert!"); // <--- Hier den Benutzernamen und das Passwort gegebenenfalls ändern
		mysqli_set_charset($db, "utf8");
		@mysqli_select_db($db, "dbgesundhait") or die("Datenbankzugriff gescheitert!");
		return $db;
	}
	
	function closeDBConnection($connection)
	{		
		mysqli_close($connection);
	}