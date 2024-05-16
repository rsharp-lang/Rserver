// export R# package module type define for javascript/typescript language
//
//    imports "http" from "RwebHost";
//
// ref=RwebHost.HttpServer@RwebHost, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace http {
   /**
   */
   function getHeaders(req: object): object;
   /**
   */
   function getHttpRaw(req: object): string;
   /**
   */
   function getUrl(req: object): object;
   /**
     * @param env default value Is ``null``.
   */
   function headers(driver: object, headers: object, env?: object): object;
   /**
     * @param silent default value Is ``true``.
   */
   function http_socket(silent?: boolean): object;
   /**
   */
   function httpError(write: object, code: object, message: string): ;
   /**
     * @param accessAny default value Is ``false``.
     * @param env default value Is ``null``.
   */
   function httpMethod(driver: object, method: string, process: any, accessAny?: boolean, env?: object): object;
   /**
     * @param port default value Is ``-1``.
     * @param env default value Is ``null``.
   */
   function listen(driver: object, port?: object, env?: object): object;
   /**
   */
   function parseUrl(url: string): object;
   /**
   */
   function pushDownload(response: object, filename: string): ;
}
