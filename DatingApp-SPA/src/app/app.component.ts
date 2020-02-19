import { Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';
import {JwtHelperService} from '@auth0/angular-jwt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'DatingApp-SPA';
  jwtHelper = new JwtHelperService();
  constructor(private auth: AuthService) {
  }

  ngOnInit() {
    const token = localStorage.getItem('token');
    const user = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.auth.decodedToken = this.jwtHelper.decodeToken(token);
    }
    if (user) {
      this.auth.currentUser = user;
      this.auth.changeMemberPhoto(user.photoUrl);
    }
  }
}
