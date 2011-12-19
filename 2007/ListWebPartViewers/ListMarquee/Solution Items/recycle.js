function recycle()
{
	try
	{
		var service = GetObject( "winmgmts:/root/MicrosoftIISv2" );
		var appPools = service.ExecQuery(
		   "SELECT * FROM IIsApplicationPool " 
		);
		
		var appPoolName;
		var appPoolEnum = new Enumerator( appPools );
		for( ; ! appPoolEnum.atEnd(); appPoolEnum.moveNext() )
		{
		   appPoolEnum.item().Recycle;   
		}

		//WScript.Echo(".IIS Recycle complete");
	}
	catch(e)
	{
		WScript.Echo("Could not recycle IIS. Please recycle it manually.");
	}
}

recycle();