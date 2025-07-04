export class MailValidatorHelper {

    public static validateMailCharacters(event: KeyboardEvent) {
    const allowedPattern = /^[a-zA-Z0-9@._-]$/;
    const key = event.key;

    if (!allowedPattern.test(key)) {
      event.preventDefault();
    }
  }

    public static validateMail(mail) {
    if (mail.length == 0) {
      return false;
    }
    //const emailPattern = /^[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)*@[a-zA-Z0-9]+(\.[a-zA-Z0-9]+)+$/;  OLD
    const emailPattern = /^[a-zA-Z0-9][a-zA-Z0-9._-]*[a-zA-Z0-9]@[a-zA-Z0-9][a-zA-Z0-9._-]*[a-zA-Z0-9]\.[a-zA-Z]{2,}$/;
    if (emailPattern.test(mail)) {
      return true;
    } else {
      return false;
    }
  }
}