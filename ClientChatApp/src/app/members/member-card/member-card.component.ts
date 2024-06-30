import { Component, Input, OnInit } from '@angular/core';
import { member } from '../../_models/member';
import { MembersService } from '../../_services/members.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrl: './member-card.component.css',
})
export class MemberCardComponent implements OnInit {
  @Input() member: any;

  photoUrl: string = '';
  constructor(
    private memberService: MembersService,
    private toaster: ToastrService
  ) {}

  ngOnInit(): void {}

  addlike(member: member) {
    this.memberService.addlikes(member.username).subscribe(() => {
      this.toaster.success('You liked ' + member.knownAs);
    });
  }
}
