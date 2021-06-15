imports "http" from "Rhttp";

# title: R# web http server
# author: xieguigang
# description: a commandline R# script for running a http web server.

[@info "the http port for listen, 80 port number is used by default."]
const httpPort as integer = ?"--listen" || 80;

http::http_socket()
|> headers(
	"X-Powered-By" = "R# server"
)
|> httpGet(function(req, response) {

})
|> httpPost(function(req, response) {

})
|> http("PUT", [req, response] => writeLines("HTTP PUT test success!", con = response))
|> listen(port = httpPort)
;