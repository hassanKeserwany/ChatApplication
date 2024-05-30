import { HttpClient } from '@angular/common/http';
import { Component, OnInit, input } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  registerMode = false;
  baseUrl: string = 'https://localhost:5001/api/';
  users: any = {};

  constructor(private http: HttpClient) {}
  ngOnInit(): void {
    this.getUser()
  }

  registerToggle() {
    this.registerMode = !this.registerMode;
  }

  //array of users
  getUser() {
    this.http
      .get(this.baseUrl + 'Users').subscribe((res) => (this.users = res));
    }


    cancelRegisterMode(event:boolean){
      this.registerMode=event;
    }

}
