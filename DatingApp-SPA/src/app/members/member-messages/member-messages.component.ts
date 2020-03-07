import { Component, OnInit, Input } from '@angular/core';
import { UserService } from 'src/app/services/user.service';
import { AuthService } from 'src/app/services/auth.service';
import { Message } from 'src/app/_models/messages';
import { AlertifyService } from 'src/app/services/alertify.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css']
})
export class MemberMessagesComponent implements OnInit {
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(private userService: UserService,
              private authService: AuthService,
              private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadData();
  }

  loadData() {
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
          .subscribe (  (data) => {
          this.messages = data;
          console.log(JSON.stringify(this.messages));
          });
  }

  sendMessage() {
    console.log(this.newMessage.content);
    this.newMessage.recipientId = this.recipientId;

    this.userService
      .sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe(
        (message: Message) => {
          this.messages.unshift(message);
          this.newMessage.content = '';
        },
        error => {
          this.alertify.error(error);
        }
      );
  }
}
