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

  canActivate(next: ActivatedRouteSnapshot): boolean {
    if (this.auth.loggedIn()) {
         // tslint:disable-next-line: no-string-literal
         const roles = next.firstChild.data['roles'] as Array<string>;
         if (roles) {
            console.log(roles);
            if (!this.auth.isRoleMatch(roles)) {
              this.alertify.error('You are not allowed to access!!!');
              this.route.navigate(['/member']);
              return false;
            }
          }
         return true;
    }

    this.alertify.error('You shall not pass!!!');

    this.route.navigate(['/home']);
  }

}
