import { Component, OnInit, Input } from '@angular/core';
import { UserService } from 'src/app/services/user.service';
import { AuthService } from 'src/app/services/auth.service';
import { Message } from 'src/app/_models/messages';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];

  constructor(private userSerice: UserService,
              private authService: AuthService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.userSerice.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
          .subscribe (  (data) => {
          this.messages = data;
          });
  }
}
