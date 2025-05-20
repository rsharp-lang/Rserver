// export R# source type define for javascript/typescript language
//
// package_source=Rserver

declare namespace Rserver {
   module _ {
      /**
      */
      function onLoad(): object;
   }
   /**
   */
   function file_transfer(localfile: any): object;
   /**
     * @param err default value Is ``500``.
   */
   function http_error(data: any, err?: any): object;
   /**
     * @param s default value Is ``null``.
   */
   function http_success(data: any, s?: any): object;
   /**
   */
   function scan_urls(): object;
}
