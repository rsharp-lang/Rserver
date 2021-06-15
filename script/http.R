imports "http" from "Rhttp";

# title: R# web http server
# author: xieguigang
# description: a commandline R# script for running a http web server.

[@info "the http port for listen, 80 port number is used by default."]
const httpPort as integer = ?"--listen" || 80;

const router as function(url) {
	`${dirname(@script)}/../web.R/${url$path}.R`;
}

const handleHttpGet as function(req, response) {
	const R as string = router(getUrl(req));

	str(getUrl(req));
	str(getHeaders(req));

	if (file.exists(R)) {
		writeLines(source(R), con = response);
	} else {
		response 
		|> httpError(404, `the required Rscript file is not found on filesystem location: '${ normalizePath(R) }'!`)
		;
	}	
}

const handleHttpPost as function(req, response) {
	str(getHeaders(req));
	writeLines("hello http post!", con = response);
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