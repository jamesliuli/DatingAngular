import { Component, OnInit, Input } from '@angular/core';
import { UserService } from 'src/app/services/user.service';
import { AuthService } from 'src/app/services/auth.service';
import { Message } from 'src/app/_models/messages';
import { AlertifyService } from 'src/app/services/alertify.service';
import { tap } from 'rxjs/operators';

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
    const currentUserId = +this.authService.decodedToken.nameid;
    this.userService.getMessageThread(this.authService.decodedToken.nameid, this.recipientId)
    .pipe(
      tap(messages => {
        // tslint:disable-next-line: prefer-for-of
        for (let i = 0; i < messages.length; i++) {
          if (
            messages[i].isRead === false &&
            messages[i].recipientId === currentUserId
          ) {
            console.log('unread msg ' + i);
            this.userService.markAsRead(currentUserId, messages[i].id)
            .subscribe( () => {
                  this.alertify.success('Mark message read #' + i);
              }, error => {
                this.alertify.error(error);
              });
          }
        }
      })
    )
    .subscribe (  messages => {
    this.messages = messages;
    // console.log(JSON.stringify(this.messages));
    },
    error => {
      this.alertify.error(error);
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
