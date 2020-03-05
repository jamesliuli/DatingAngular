import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { UserService } from 'src/app/services/user.service';
import { AlertifyService } from 'src/app/services/alertify.service';
import { PaginatedResult, Pagination } from 'src/app/_models/pagination';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  users: User[];
  // userParams: UserParams = { CurrentPage : 1, PageSize: 5};  // page# start from 1
  pagination: Pagination;

  constructor(private userService: UserService, private alertify: AlertifyService,
              private route: ActivatedRoute) { }

  ngOnInit() {
    // this.loadUsers();

    this.route.data.subscribe(data => {
      this.users = data['users'].result;
      this.pagination = data['users'].pagination;
      console.log(JSON.stringify(this.pagination));
    });
  }
  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    console.log(event.page);
    this.loadUsers();
  }

  loadUsers() {
    this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage)
    .subscribe( (result: PaginatedResult<User[]>) => {
      this.users = result.result;
      this.pagination = result.pagination;

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

  // }
}
