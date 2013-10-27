//
//  ММMyStatsViewController.m
//  moodmap
//
//  Created by Roman Putsykovich on 10/27/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import "ММMyStatsViewController.h"
#import <FacebookSDK/FacebookSDK.h>
#import "ММMoodState.h"

@interface __MyStatsViewController ()

@property (weak, nonatomic) IBOutlet FBProfilePictureView *profilePicture;
@property (weak, nonatomic) IBOutlet UILabel *statusLabel;
@property (weak, nonatomic) IBOutlet UIImageView *moodImage;
@property (weak, nonatomic) IBOutlet UITableView *table;
@property (weak, nonatomic) IBOutlet UIImageView *chartImage;

@property (strong, nonatomic) NSArray *moodHistory;

@end

@implementation __MyStatsViewController

static NSString * const FacebookUserName = @"FBUserName";
static NSString * const FacebookProfileID = @"FBProfileID";
static NSString * const MoodImageKey = @"MoodImageKey";

- (void)viewDidLoad {
    [super viewDidLoad];
	// Do any additional setup after loading the view.
    
    self.moodHistory = [self getMoodHistory];
}

- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    
    NSString *profileID = [[NSUserDefaults standardUserDefaults] objectForKey:FacebookProfileID];
    self.profilePicture.profileID = profileID;
    self.statusLabel.text = @"You're doing great, keep it up!";
    if([[NSUserDefaults standardUserDefaults] objectForKey:MoodImageKey]) {
        self.moodImage.image = [UIImage imageNamed:[[NSUserDefaults standardUserDefaults] objectForKey:MoodImageKey]];
    }
}

- (IBAction)segmentedControlChangedValue:(UISegmentedControl *)sender {
    UISegmentedControl * segmentedControl = (UISegmentedControl *)sender;
    switch (segmentedControl.selectedSegmentIndex) {
        case 0:
            self.chartImage.image =[UIImage imageNamed:@"day.png"];
            break;
        case 1:
            self.chartImage.image =[UIImage imageNamed:@"week.png"];
            break;
        case 2:
            self.chartImage.image =[UIImage imageNamed:@"alltime.png"];
            break;
        default:
            break;
    }
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
    return [self.moodHistory count];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
    static NSString *CellIdentifier = @"MoodHistoryCell";
    UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier forIndexPath:indexPath];
    
    
    if (!cell) {
        cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
    }
    
    __MoodState *moodState = [self.moodHistory objectAtIndex:indexPath.row];
    cell.textLabel.text = moodState.mood;
    cell.detailTextLabel.text = moodState.location;
    cell.imageView.image = [UIImage imageNamed:moodState.moodImage];
    
    return cell;
}


- (NSArray *)getMoodHistory{
    
    NSMutableArray *array = [[NSMutableArray alloc] init];
    
    __MoodState *moodState = [[__MoodState alloc] init];
    moodState.mood = @"Feeling Awesome";
    moodState.location = @"New York, NY";
    moodState.moodImage = @"awesome32.png";
    moodState.timeAgo = @"15m";
    [array addObject:moodState];
    
    moodState = [[__MoodState alloc] init];
    moodState.mood = @"Feeling Sleepy";
    moodState.location = @"Patio Pizzeria, Belarus";
    moodState.moodImage = @"sleepy32.png";
    moodState.timeAgo = @"1h";
    [array addObject:moodState];
    
    moodState = [[__MoodState alloc] init];
    moodState.mood = @"Feeling Determined";
    moodState.location = @"Moscow, Russia";
    moodState.moodImage = @"determined32.png";
    moodState.timeAgo = @"2d";
    [array addObject:moodState];
    
    moodState = [[__MoodState alloc] init];
    moodState.mood = @"Feeling Better";
    moodState.location = @"Moscow, Russia";
    moodState.moodImage = @"better32.png";
    moodState.timeAgo = @"2d";
    [array addObject:moodState];
    
    return array;
}



@end
