import { Component, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';
import { member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { Observable, Subject, every, take } from 'rxjs';
import { Pagination } from '../../_models/pagination';
import { UserParams } from '../../_models/userParams';
import { AccountService } from '../../_services/account.service';
import { User } from '../../_models/User';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  baseUrl: string = environment.apiUrl;
  members!: member[];
  pagination!: Pagination;
  user!: User;
  filtering: boolean = false;
  genderList = [
    { value: 'male', display: 'Males' },
    { value: 'female', display: 'Females' },
  ];

  userParams: UserParams;

  constructor(private memberService: MembersService ) {
    this.userParams =this.memberService.getUserParams();
    }
 
  ngOnInit(): void {
    this.LoadMembers();
  }
 
  LoadMembers() {
    this.memberService.setUserParams(this.userParams);
    this.memberService.getMembers(this.userParams).subscribe(
      (res) => {
        this.members =  res.result!;
        this.pagination = res.pagination!;
      },
      (error) => {
        console.log(error);
      }
    );
  }
  resetFilters() {
    this.userParams =this.memberService.resetUserParams();
    this.LoadMembers()
  }
  filterData() {
    this.filtering = !this.filtering;
  }

  pageChanged(event: any) {
    this.userParams.pageNumber = event.page;
    this.memberService.setUserParams(this.userParams)
    this.LoadMembers();
  }


  
}
