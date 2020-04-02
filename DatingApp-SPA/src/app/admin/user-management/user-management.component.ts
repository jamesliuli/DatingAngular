import { Component, OnInit } from '@angular/core';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/services/admin.service';
import { BsModalService, BsModalRef } from 'ngx-bootstrap';
import { RolesModalComponent } from '../roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: User[];
  bsModalRef: BsModalRef;
  constructor(private adminService: AdminService,
              private modalService: BsModalService) { }

  ngOnInit() {
    this.getUsersWithRoles();
  }

  getUsersWithRoles() {
    this.adminService.getUsersWithRoles().subscribe( (users: User[]) => {
      this.users = users;
      console.log(users);
    }, error => {
      console.log(error);
    } );
  }

  editUserRoles(user: User) {
    const initialState = {
        user,
        roles: this.getUserEditRoles(user)
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
    this.bsModalRef.content.closeBtnName = 'Close';
  }

  getUserEditRoles(user: User): any {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] = [
      {name: 'Admin', value: 'Admin' },
      {name: 'Moderator', value: 'Moderator' },
      {name: 'Member', value: 'Member' },
      {name: 'VIP', value: 'VIP' }
    ];

    availableRoles.forEach(ar => {
      let isMatch = false;
      userRoles.forEach( r => {
          if (ar.name === r) {
             roles.push({name: ar.name, checked: true});
             isMatch = true;
             return;
          }
      });

      if (!isMatch) {
        roles.push({name: ar.name, checked: false});
      }
    });

    return roles;
  }

}
