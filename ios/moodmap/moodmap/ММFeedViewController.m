//
//  ММFeedViewController.m
//  moodmap
//
//  Created by Roman Putsykovich on 10/26/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import "ММFeedViewController.h"
#import "ММFeedCell.h"
#import <FacebookSDK/FacebookSDK.h>

@interface __FeedViewController () <FBFriendPickerDelegate>

@property (strong, nonatomic) NSArray *friendsArray;
@property (strong, nonatomic) IBOutlet UITableView *table;

@end

@implementation __FeedViewController

- (void)viewDidLoad {
    [super viewDidLoad];
}

- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    
    FBRequest* friendsRequest = [FBRequest requestForMyFriends];
    [friendsRequest startWithCompletionHandler: ^(FBRequestConnection *connection,
                                                  NSDictionary* result,
                                                  NSError *error) {
        NSArray* friends = [result objectForKey:@"data"];
        self.friendsArray = friends;
        [self.table reloadData];
        
        //NSLog(@"Found: %lu friends", (unsigned long)friends.count);
        //for (NSDictionary<FBGraphUser>* friend in friends) {
           //NSLog(@"I have a friend named %@ with id %@", friend.name, friend.id);
        //}
    }];
}

- (void)didReceiveMemoryWarning {
    [super didReceiveMemoryWarning];
    // Dispose of any resources that can be recreated.
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [self.friendsArray count];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    static NSString *CellIdentifier = @"FeedCell";
    __FeedCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier forIndexPath:indexPath];
   
    NSDictionary<FBGraphUser> *friend = [self.friendsArray objectAtIndex:indexPath.row];
    cell.profilePicture.profileID = friend.id;
    cell.nameLabel.text = friend.name;

    //id<FBGraphPlace> place = friend.location;
    //id<FBGraphLocation> location = place.location;
    //cell.locationLabel.text = [NSString stringWithFormat:@"%@, %@", location.city, location.state];//friend.location[@"name"];
    cell.moodLabel.text = @"Feeling Awesome";
    return cell;
}

@end
