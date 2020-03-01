import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { User } from '../_models/user';
import { UserService } from '../services/user.service';
import { AlertifyService } from '../services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '../_models/messages';
import { AuthService } from '../services/auth.service';

@Injectable()
export class MessageResolver implements Resolve<Message[]> {
messageContainer = 'Unread';

    constructor(private userSerive: UserService,
                private authService: AuthService,
                private router: Router,
                private alertify: AlertifyService) {
                }

    resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
        return this.userSerive.getMessages(this.authService.decodedToken.nameid,
            this.messageContainer).pipe(
            catchError(error => {
                this.alertify.error(error);
                this.router.navigate(['/Home']);
                return of(null);
            })
        );
    }
}