import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  constructor(public auth: AuthService, private alertifyServer: AlertifyService,
              private route: Router) { }


  ngOnInit() {
  }

  login() {
    // console.log(this.model);
    this.auth.login(this.model).subscribe( next => {
      this.alertifyServer.success('login successfully.');
    }, error => {
      this.alertifyServer.error('failed to login');
    },
    () => {
      this.route.navigate(['/members']);
    });
  }

  loggedIn()  {
    return this.auth.loggedIn();
  }

  logout()  {
    localStorage.removeItem('token');
    this.alertifyServer.message('logged out');
    this.route.navigate(['/home']);
  }
}
