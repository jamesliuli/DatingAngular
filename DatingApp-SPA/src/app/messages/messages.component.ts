import { Component, OnInit } from '@angular/core';
import { Message } from '../_models/messages';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {

  constructor(private userService: UserService,
              private authService: AuthService,
              private route: ActivatedRoute) { }
  messages: Message[];
  messageContainer = 'Unread';

  ngOnInit() {
    this.route.data.subscribe( data => {
      // tslint:disable-next-line: no-string-literal
      this.messages = data['messages'];
      console.log(JSON.stringify(this.messages));
    });
  }

  loadMessages() {
    this.userService.getMessages(this.authService.decodedToken.nameid,
      this.messageContainer).subscribe ( (data: Message[]) => {
        this.messages = data;
      });
  }

  deleteMessage(id: number) {
    console.log(id);
  }
}
