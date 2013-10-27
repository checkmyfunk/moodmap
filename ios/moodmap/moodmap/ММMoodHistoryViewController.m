//
//  ММMoodHistoryViewController.m
//  moodmap
//
//  Created by Roman Putsykovich on 10/27/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import "ММMoodHistoryViewController.h"
#import "ММMoodState.h"

@interface __MoodHistoryViewController ()

@property (weak, nonatomic) IBOutlet FBProfilePictureView *profilePicture;
@property (weak, nonatomic) IBOutlet UILabel *statusLabel;
@property (weak, nonatomic) IBOutlet UITableView *table;

@property (strong, nonatomic) NSArray *moodHistory;

@end

@implementation __MoodHistoryViewController

- (void)viewDidLoad {
    [super viewDidLoad];
	// Do any additional setup after loading the view.
    
    self.moodHistory = [self getMoodHistory];
}

- (void)viewWillAppear:(BOOL)animated {
    [super viewWillAppear:animated];
    
    self.navigationController.visibleViewController.title = self.user.name;
    self.profilePicture.profileID = self.user.id;
    self.statusLabel.text = [NSString stringWithFormat:@"%@ is doing great most of the time.", self.user.first_name];
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
    moodState.mood = @"Feeling Awesome";
    moodState.location = @"Patio Pizzeria, Belarus";
    moodState.moodImage = @"awesome32.png";
    moodState.timeAgo = @"1h";
    [array addObject:moodState];
    
    moodState = [[__MoodState alloc] init];
    moodState.mood = @"Feeling Sleepy";
    moodState.location = @"Moscow, Russia";
    moodState.moodImage = @"sleepy32.png";
    moodState.timeAgo = @"4h";
    [array addObject:moodState];
    
    moodState = [[__MoodState alloc] init];
    moodState.mood = @"Feeling Determined";
    moodState.location = @"Moscow, Russia";
    moodState.moodImage = @"determined32.png";
    moodState.timeAgo = @"2d";
    [array addObject:moodState];
    
    moodState = [[__MoodState alloc] init];
    moodState.mood = @"Feeling Awesome";
    moodState.location = @"Moscow, Russia";
    moodState.moodImage = @"awesome32.png";
    moodState.timeAgo = @"4d";
    [array addObject:moodState];
    
    moodState = [[__MoodState alloc] init];
    moodState.mood = @"Feeling Bad";
    moodState.location = @"Moscow, Russia";
    moodState.moodImage = @"bad32.png";
    moodState.timeAgo = @"7d";
    [array addObject:moodState];
    
    return array;
}


@end
