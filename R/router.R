imports "router" from "RwebHost";

#' scan runtime for web app register
#' 
const scan_urls = function() {
    let current_env <- environment(); 
    let all_functions <- ls("Function", envir = current_env);
    let http_tools <- all_functions |> which(function(f) {
        let attrs = .Internal::attributes(f);
        let url = attrs$url;

        nchar(url) > 0; 
    });
    let router = router::new();

    for(let func in http_tools) {
        # [@url "/tool_name"]
        # [@http "get/post"]
        let attrs = .Internal::attributes(func);
        let url = attrs$url;
        # default method is http get if the
        # @http custom attribute is missing 
        let http_method = tolower(attrs$http) || "get";

        router |> register_url(
                url = url,
             method = http_method,
            handler = func
        );
    }

    return(router);
}