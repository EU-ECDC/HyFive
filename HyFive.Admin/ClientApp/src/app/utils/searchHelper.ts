import { User } from "../models/api/User";

export class SearchHelper {
    public static filterUsers(searchWord: string, usersList: User[]) : User[] {
        
            var filteredUsers = usersList.filter(k => 
                                        k.firstName?.toLowerCase().includes(searchWord.toLowerCase()) || 
                                        k.lastName?.toLocaleLowerCase().includes(searchWord.toLowerCase()) ||
                                        (k.firstName + ' ' + k.lastName).toLowerCase().includes(searchWord.toLowerCase()) ||
                                        k.hprNumber?.includes(searchWord));

            return filteredUsers;
    }
}