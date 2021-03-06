import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {BehaviorSubject} from 'rxjs';
import { map } from 'rxjs/operators';
import {JwtHelperService} from '@auth0/angular-jwt';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

// baseUrl = 'http://localhost:5000/api/auth/';
baseUrl = environment.apiUrl + 'auth/';
jwtHelper = new JwtHelperService();
decodedToken: any;
currentUser: User;
photoUrl = new BehaviorSubject<string>('../../assets/user.png');
currentPhotoUrl = this.photoUrl.asObservable();

loginUserChangedSubject = new BehaviorSubject<boolean>(false);
loginUserChanged = this.loginUserChangedSubject.asObservable();

constructor(private http: HttpClient) { }

changeMemberPhoto(photoUrl: string) {
  this.photoUrl.next(photoUrl);
}

login(model: any) {
    console.log(model);
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map( (response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.decodedToken = this.jwtHelper.decodeToken(user.token);
          console.log(this.decodedToken);

          this.currentUser = user.user;
          this.changeMemberPhoto(this.currentUser.photoUrl);
          this.loginUserChangedSubject.next(true);
        }
      }
    ));
  }

  register(user: User) {
    return this.http.post(this.baseUrl + 'register', user);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }

  isRoleMatch(requiredRoles: Array<string>): boolean {
    let isMatch = false;
    const roles = this.decodedToken.role as Array<string>;
    console.log(roles);
    requiredRoles.forEach(element => {
      if (roles.includes(element)) {
        isMatch = true;
        return;
      }
    });
    return isMatch;
  }
}
