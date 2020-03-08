import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/messages';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../services/alertify.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  constructor(private userService: UserService,
              private authService: AuthService,
              private route: ActivatedRoute,
              private alertify: AlertifyService) { }
  messages: Message[];
  messageContainer = 'Unread';

  ngOnInit() {
    this.route.data.subscribe( data => {
      // tslint:disable-next-line: no-string-literal
      this.messages = data['messages'];
      // console.log(JSON.stringify(this.messages));
    });
  }

  loadMessages() {
    this.userService.getMessages(this.authService.decodedToken.nameid,
      this.messageContainer).subscribe ( (data: Message[]) => {
        this.messages = data;
      });
  }

  deleteMessage(id: number) {
    this.userService.deleteMessage(this.authService.decodedToken.nameid, id)
    .subscribe( () => {
        const idx = this.messages.findIndex( m => m.id === id);
        console.log('idx of msg ' + idx);
        this.messages.splice( idx, 1);
        this.alertify.success('Deleted');
      }, error => {
        this.alertify.error(error);
      });
  }
}
