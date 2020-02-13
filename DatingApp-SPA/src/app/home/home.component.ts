import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
  valuesFromHome: any = {};
  showRegister = false;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getValues();
  }

  registerToggle() {
      this.showRegister = !this.showRegister;
  }

  getValues()  {
    this.http.get('http://localhost:5000/api/values').subscribe( response => {
      this.valuesFromHome = response;
    },
    error => {
      console.log(error);
    } );
  }
}
