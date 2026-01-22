import { Injectable } from "@angular/core";
import { TranslateService } from "@ngx-translate/core";

@Injectable({
  providedIn: 'root'
})
export class DialogMessageService {
    constructor(private readonly translate: TranslateService) {

    }

    translateDialogMessage (text: string, id: number): string {
        let message =  this.translate.instant(text);
        message = message + ' ' + id + "?";
        return message;
    }

    translateText(text: string): string {
        return this.translate.instant(text);
    }
    
}