//
//  ММMoodHistoryViewController.h
//  moodmap
//
//  Created by Roman Putsykovich on 10/27/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import <UIKit/UIKit.h>
#import <FacebookSDK/FacebookSDK.h>

@interface __MoodHistoryViewController : UIViewController <UITableViewDataSource, UITableViewDelegate>

@property (strong, nonatomic) NSDictionary<FBGraphUser> *user;

@end
