import { Component, OnInit } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

const httpOptions = {
  headers: new HttpHeaders({
    Authorization: 'Bearer ' + localStorage.getItem('token')
  })
};

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
    this.http.get('http://localhost:5000/api/values', httpOptions).subscribe( response => {
      this.valuesFromHome = response;
      console.log(this.valuesFromHome);
    },
    error => {
      console.log(error);
    } );
  }
}
