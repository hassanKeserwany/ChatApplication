import { Component, OnInit } from '@angular/core';
import { member } from '../_models/member';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { MembersService } from '../_services/members.service';
import { PageChangedEvent } from 'ngx-bootstrap/pagination';
import { Pagination } from '../_models/pagination';
import { UserParams } from '../_models/userParams';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrl: './lists.component.css',
})
export class ListsComponent implements OnInit {
  members!: Partial<member[]>; //partial means some of the properties of the member objects in the array might be missing.
  predicate: string = 'liked';
  pagination!: Pagination;
  userParams: UserParams = new UserParams();
  pageNumber = 1;
  pageSize = 5;
  constructor(private memberService: MembersService) {}

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes() {
    this.memberService
      .getLikes(this.predicate, this.pageNumber, this.pageSize)
      .subscribe((response) => {
        this.members = response.result;
        this.pagination=response.pagination;
        
      });
  }
  pageChanged(event: any) {
    this.pageNumber = event.page;
    this.memberService.setUserParams(this.userParams);
    this.loadLikes();
  }
}
