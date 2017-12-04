<%
    Response.StatusCode = 500;
    Response.TrySkipIisCustomErrors = true;
%>

<!doctype html>
<html lang="en">
<head>
    <title>An Error Occurred</title>
</head>
<body>
    <h1>An Error Occurred</h1>
    <h4>Our server couldn't complete your request.
        <br />
        Please refresh your browser and try again soon.
    </h4>
    <a href="/">Home</a>
    <p>
        Error Code: 500
    </p>
</body>
</html>