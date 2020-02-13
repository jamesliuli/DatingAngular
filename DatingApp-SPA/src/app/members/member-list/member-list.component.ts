import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/services/user.service';
import { AlertifyService } from 'src/app/services/alertify.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  users: User[];

  constructor(private userService: UserService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers().subscribe( (users: User[]) => {
      this.users = users;
    },
    error => {
      this.alertify.error(error);
    });
  }

  // loadUser(id: number){
  //   this.userService.getUser(id).subscribe( next => {
  //     this.user = this.user;
  //   },
  //   error => {
  //     this.alertify.error(error);
  //   });

  //}
}
