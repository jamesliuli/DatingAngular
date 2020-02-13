import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { AlertifyService } from '../services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private auth: AuthService,
              private route: Router,
              private alertify: AlertifyService) {
  }

  canActivate(): boolean {
    if (this.auth.loggedIn()) {
         return true;
    }

    this.alertify.error('you shall not pass!!!');

    this.route.navigate(['/home']);
  }

}
