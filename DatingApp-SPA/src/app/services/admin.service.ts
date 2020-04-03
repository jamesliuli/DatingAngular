import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  constructor(private http: HttpClient) { }

  getUsersWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/userWithRoles');
  }

  updateUserRoles(user: User, roles: {}) {
    return this.http.post(this.baseUrl + 'admin/editroles/' + user.userName, roles);
  }
}
