<?php
	include 'sqlconnect.php';
	include 'userFunctions.php';
	
		dropFile();

	// Get the HTTP method, path and body of the request
	$httpMethod = $_SERVER['REQUEST_METHOD'];
	$httpPath = explode('/', trim($_SERVER['PATH_INFO'],'/'));
	$httpInput = json_decode(file_get_contents('php://input'),true);
	 
	// Retrieve the table and key from the path
	$httpTable = preg_replace('/[^a-z0-9_]+/i','',array_shift($httpPath));
	$httpKey = array_shift($httpPath)+0;
	 	
	// Check if the request is correct
	if(empty($httpTable) || $httpTable != $httpTableUsers)
	{
	  http_response_code(404);
	  die("Wrong HTTP Rest Request!");		
	}
		
	// On POST, PUT and DELETE create String for SQL set
echo($httpInput);
echo($httpTable);
	if(!empty($httpInput))
	{
		if($httpTable == $httpTableUsers)
		{
			$set = getUserSQLSetStringFromHttpInput($httpInput);			
		}
		if($httpTable == "file")
		{
			echo($httpInput);
			downloadFile($httpInput);
			die();
		}
	}	 
	 
	// Connect to the mysql database
	$dbConnection = getDBConnection();
	
	// Create SQL based on HTTP method
	switch ($httpMethod) {
	  case 'GET':
		$sqlStatement = "SELECT * FROM `$httpTable`".($httpKey?" WHERE ID = $httpKey":'');
		break;
	  case 'PUT':
		$sqlStatement = "UPDATE `$httpTable` SET $set WHERE ID = $httpKey";
		break;
	  case 'POST':
		$sqlStatement = "INSERT INTO `$httpTable` SET $set";
		break;
	}
	 
	// Excecute SQL statement
	$sqlResult = mysqli_query($dbConnection,$sqlStatement);
	 
	// Die if SQL Statement failed
	if (!$sqlResult)
	{
	  http_response_code(404);
	  die("SQL Statement Failed!");
	}
	
	// Print results, insert id or affected row count
	if ($httpMethod == 'GET') 
	{
		if($httpTable == $httpTableUsers)
		{
			outputUsersResult($httpKey, $sqlResult);		
		}
	} 
	elseif ($httpMethod == 'POST') echo mysqli_insert_id($dbConnection);
	else echo mysqli_affected_rows($dbConnection);// PUT, DELETE etc. 
	 
	// Close MySql Connection
	closeDBConnection($dbConnection);
