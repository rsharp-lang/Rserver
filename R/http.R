require(JSON);

#' push local file for download
#' 
const file_transfer = function(localfile) {
    pushDownload(localfile);
}

const http_success = function(data, s = NULL) {
    let response = {
        code: 0,
        info: data
    };

    if (sink.number() > 0) {
        sink();
    }

    s <- ifelse(is.null(s), buffer("text", 
        mime = "application/json"), s);

    response
    |> JSON::json_encode(row.names = TRUE,
        unicode.escape = FALSE)
    |> writeLines(con = s)
    ;
}

const http_error = function(data, err = 500) {
    let response = {
        code: err,
        info: data
    };

    if (sink.number() > 0) {
        sink();
    }

    response
    |> JSON::json_encode(row.names = TRUE, 
        unicode.escape = FALSE)
    |> writeLines(con = buffer("text", 
        mime = "application/json"))
    ;
}