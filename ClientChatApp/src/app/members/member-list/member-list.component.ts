import { Component, OnInit } from '@angular/core';
import { environment } from '../../../environments/environment';
import { member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrl: './member-list.component.css',
})
export class MemberListComponent implements OnInit {
  baseUrl: string = environment.apiUrl;
  members$!: Observable<member[]>;
  constructor(private memberService: MembersService) {}
  ngOnInit(): void {
    this.members$= this.memberService.getMembers();
  }

  // LoadMembers() {
  //   this.memberService.getMembers().subscribe((res) => {
  //     this.members = res;
  //     //console.log(res);
  //   },error=>{
  //     console.log(error);
  //   })
  // }
}
