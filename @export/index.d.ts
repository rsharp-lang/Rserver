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
   */
   function http_success(data: any): object;
}
