import { Component, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { UserService } from '../services/user.service';
import { AlertifyService } from '../services/alertify.service';
import { User } from '../_models/user';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {

  users: User[];
  likesParam = 'Likers';
  constructor(private authService: AuthService,
              private userService: UserService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadLikes();
  }

  loadLikes() {
    let liker = false;
    if (this.likesParam === 'Likers') {
      liker = true;
    } else {
      liker = false;
    }
    console.log(this.likesParam);

    this.userService.getLikers(this.authService.decodedToken.nameid, liker).subscribe( (users: User[]) => {
      this.users = users;
    },
    error => {
      this.alertify.error(error);
    });
  }
}
