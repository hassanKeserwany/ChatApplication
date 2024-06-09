import { Component, OnInit } from '@angular/core';
import { member } from '../_models/member';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrl: './lists.component.css',
})
export class ListsComponent implements OnInit {


constructor(){
  
}
  ngOnInit (): void {
  }

}
