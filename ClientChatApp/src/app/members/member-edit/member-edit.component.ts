import {
  Component,
  HostListener,
  OnInit,
  ViewChild,
  viewChild,
} from '@angular/core';
import { member } from '../../_models/member';
import { User } from '../../_models/User';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';
import { take } from 'rxjs';
import { ToastrService } from 'ngx-toastr';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrl: './member-edit.component.css',
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm!: NgForm;
  member!: member;
  user!: any;
  //if we want to edit member, and accedently move to another site(ex:google.com),it notify us
  @HostListener('window:beforeunload', ['event']) unloadNotification(
    $event: any
  ) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private accountService: AccountService,
    private memberService: MembersService,
    private toast: ToastrService
  ) {
    //we take the user found in currentUser$ , which is logged in now
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.user = user;
      this.loadMember();
    });
  }
  ngOnInit(): void {}
  loadMember() {
    this.memberService.getMember(this.user.userName).subscribe((res) => {
      this.member = res;
    });
  }

  updateMember() {
    this.memberService.updateMember(this.member).subscribe(() => {
      this.toast.success('Updated Successfully ...');
      this.editForm.reset(this.member);
    });
  }
}
