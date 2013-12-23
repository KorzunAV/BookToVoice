
FOR /R %%f IN (.) DO (
	if "%%~nf"=="obj" (
		rmdir /S /Q "%%f"
	)
	if "%%~nf"=="bin"	(
		rmdir /S /Q "%%f"
	)
	if "%%~nf"=="Bin"	(
		rmdir /S /Q "%%f"
	)
	if "%%~nf"=="CVS"	(
		rmdir /S /Q "%%f"
	)
	if "%%~nf"=="TestResults"	(
		rmdir /S /Q "%%f"
	)
  if "%%~nf"=="svn"	(
		rmdir /S /Q "%%f"
	)
)