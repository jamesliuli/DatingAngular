import { Component, OnInit, EventEmitter, Output, Input } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  @Input() values: any;
  @Output() CancelRegister = new EventEmitter();

  model: any = {};


  constructor(public authService: AuthService) { }

  ngOnInit() {
  }

  register() {
    this.authService.register(this.model).subscribe( () => {
      console.log('register successfully');
    }, error => {
      console.log(error);
    });
  }

  cancel() {
    console.log('cancelled');
    this.CancelRegister.emit();
  }
}
