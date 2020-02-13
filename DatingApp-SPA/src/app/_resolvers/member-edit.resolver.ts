import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../services/user.service';
import { AlertifyService } from '../services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../services/auth.service';

@Injectable()
export class MemberEditResolver implements Resolve<User> {
    constructor(private userSerive: UserService,
                private router: Router,
                private alertify: AlertifyService,
                private authService: AuthService) {
                 }

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userSerive.getUser(this.authService.docodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('Problem retrieving user data');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}
