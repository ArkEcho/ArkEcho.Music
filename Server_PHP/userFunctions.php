<?php	
	// Vars defining the JSON Keys
	$httpTableUsers = 'users';
	$JSON_Users = 'Users';
	$JSON_User = 'User';
	
	// Convert SQL Result-User to User Object; int and boolean casting
	function outputUsersResult($httpKey, $sqlResult)
	{	
		global $JSON_Users;
		global $JSON_User;		
		if (!$httpKey) echo "{\"$JSON_Users\":[";
		$rowCount = mysqli_num_rows($sqlResult);
		for ($i=0;$i<$rowCount;$i++) 
		{
			$user = mysqli_fetch_object($sqlResult);
			$user->ID = (int)$user->ID;
			$user->u_active = ($user->u_active == 1?true:false);
			if($i > 0) echo ",";
			echo "{"."\"$JSON_User\":".json_encode($user)."}";
		}			
		if (!$httpKey) echo "]}";
	}

	function dropFile()
	{
		header("Content-Type: text/plain");
		header("Content-Disposition: attachment; filename=\"test.txt\"");

		readFile('/var/www/html/test.txt');
	}
	
	function getUserSQLSetStringFromHttpInput($httpInput)
	{	
		global $JSON_User; // Using Global to get the var into the function
		$set = '';
		$userSurname = 'u_surname';
		$userForename = 'u_forename';
		$userNumber = 'u_number';
		$userMail = 'u_mail';
		$userUsername = 'u_username';
		$userPassword = 'u_password';
		$userActive = 'u_active';
		$set.= "$userSurname = \"".$httpInput[$JSON_User][$userSurname]."\", ";
		$set.= "$userForename = \"".$httpInput[$JSON_User][$userForename]."\", ";
		$set.= "$userNumber = \"".$httpInput[$JSON_User][$userNumber]."\", ";
		$set.= "$userMail = \"".$httpInput[$JSON_User][$userMail]."\", ";
		$set.= "$userUsername = \"".$httpInput[$JSON_User][$userUsername]."\", ";
		$set.= "$userPassword = \"".$httpInput[$JSON_User][$userPassword]."\", ";
		$set.= "$userActive = ".($httpInput[$JSON_User][$userActive]==1?"true":"false");
		return $set;
	}
