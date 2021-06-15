imports "http" from "Rhttp";

# title: R# web http server
# author: xieguigang
# description: a commandline R# script for running a http web server.

[@info "the http port for listen, 80 port number is used by default."]
const httpPort as integer = ?"--listen" || 80;

const handleHttpGet as function(req, response) {
	print(req);
	writeLines("hello!", con = response);
}

const handleHttpPost as function(req, response) {
	print(req);
	writeLines("hello!", con = response);
}

http::http_socket()
|> headers(
	"X-Powered-By" = "R# server"
)
|> httpMethod("GET", handleHttpGet)
|> httpMethod("POST", handleHttpPost)
|> httpMethod("PUT", [req, response] => writeLines("HTTP PUT test success!", con = response))
|> listen(port = httpPort)
;