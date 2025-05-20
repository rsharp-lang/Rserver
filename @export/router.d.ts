// export R# package module type define for javascript/typescript language
//
//    imports "router" from "RwebHost";
//
// ref=RwebHost.RouterFunction@RwebHost, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
*/
declare namespace router {
   /**
   */
   function check_url(router: object, req: object): boolean;
   /**
     * @param env default value Is ``null``.
   */
   function handle(router: object, req: object, response: object, env?: object): any;
   /**
   */
   function new(): object;
   /**
     * @param env default value Is ``null``.
   */
   function parse(exp: object, env?: object): any;
   /**
     * @param env default value Is ``null``.
   */
   function register_url(router: object, url: string, method: string, handler: any, env?: object): any;
}
