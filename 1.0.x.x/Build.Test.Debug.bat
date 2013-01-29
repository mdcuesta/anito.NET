call "C:\Program Files\Microsoft Visual Studio 10.0\VC\vcvarsall.bat"

echo Building Anito.Test.2010.csproj
echo.
devenv /build debug "Anito.Test\Anito.Test.2010.csproj"
echo. 
echo Done Building Anito.Test.2010.csproj
echo.

echo Building Anito.Test.SqlClient.2010.csproj
echo.
devenv /build debug "Anito.Test.SqlClient\Anito.Test.SqlClient.2010.csproj"
echo. 
echo Done Building Anito.Test.SqlClient.2010.csproj
echo.

echo Building Anito.Test.MySqlClient.2010.csproj
echo.
devenv /build debug "Anito.Test.MySqlClient\Anito.Test.MySqlClient.2010.csproj"
echo. 
echo Done Building Anito.Test.MySqlClient.2010.csproj
echo.

echo Building Anito.Test.SqliteClient.2010.csproj
echo.
devenv /build debug "Anito.Test.SqliteClient\Anito.Test.SqliteClient.2010.csproj"
echo. 
echo Done Building Anito.Test.SqliteClient.2010.csproj
echo.

pause