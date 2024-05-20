require(JSON);

#' push local file for download
#' 
const file_transfer = function(localfile) {
    pushDownload(localfile);
}

const http_success = function(data) {
    let response = {
        code: 0,
        info: data
    };

    response
    |> JSON::json_encode(row.names = TRUE)
    |> writeLines(con = buffer("text", mime = "application/json"))
    ;
}

const http_error = function(data, err = 500) {
    let response = {
        code: err,
        info: data
    };

    response
    |> JSON::json_encode(row.names = TRUE)
    |> writeLines(con = buffer("text", mime = "application/json"))
    ;
}