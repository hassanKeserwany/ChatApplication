import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  title = 'ClientChatApp';
  users:any;

  /**
   *
   */
  constructor(private http: HttpClient) {}
  ngOnInit() {
    this.getUsers();
  }

  getUsers() {
    this.http.get('https://localhost:5001/api/Users').subscribe(
      (respose ) => {
        this.users = respose;
        console.log(this.users);
      },
      (error) => {
        console.log(error);
      }
    );
  }
}
interface user{
  id:number ,
  name:string
}