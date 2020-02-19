import { Component, OnInit, HostListener, ViewChild } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/services/user.service';
import { AlertifyService } from 'src/app/services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { NgForm } from '@angular/forms';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm: NgForm;

  user: User;

  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
              private userService: UserService,
              private authService: AuthService,
              private alertify: AlertifyService,
              private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.data.subscribe( data => {
      // tslint:disable-next-line: no-string-literal
      this.user = data['user'];
    });

  }

  updateUser() {
    console.log( this.user);

    this.userService.updateUser(this.authService.decodedToken.nameid, this.user).subscribe( next => {
        this.alertify.success('User profile uodated');
    }, error => {
      this.alertify.error(error);
    });

  }

  Cancel() {

  }

  onMemberPhotoChanged(url: string) {
    this.user.photoUrl = url;
    console.log(url);
  }
}
