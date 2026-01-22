import { map, Observable } from "rxjs";
import { ajax, AjaxResponse } from "rxjs/ajax";
import { DownloadExcelModel } from "../models/api/downloadExcelModel";

export class DownloadFileHelper {
    public static downloadFile(url: string, acceptHeader: string, payload: DownloadExcelModel): Observable<any> {
        return ajax({
          url,
          method: 'POST',
          responseType: 'blob',
          body: {
            ...payload
          },
          headers: {
            'Content-Type': 'application/json',
            Accept: acceptHeader,
            'Cache-Control': 'no-cache',
          }
        }).pipe(
          map(this.handleDownloadSuccess),
        );
      }
    
      private static async handleDownloadSuccess(response: AjaxResponse<any>) {
        const downloadLink = document.createElement('a');
        const isIEOrEdge = /msie\s|trident\/|edge\//i.test(globalThis.navigator.userAgent);
        let filename = '';
        downloadLink.href = globalThis.URL.createObjectURL(response.response);
        const disposition = response.xhr.getResponseHeader('Content-Disposition');
        if (disposition) {
          const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
          const matches = filenameRegex.exec(disposition);
          if (matches != null && matches[1]) {
            filename = matches[1].replace(/['"]/g, '');
            downloadLink.setAttribute('download', filename);
          }
        }
        if (isIEOrEdge) {
          return true;
        } else {
          document.body.appendChild(downloadLink);
          downloadLink.click();
          document.body.removeChild(downloadLink);
        }
      }
    
}