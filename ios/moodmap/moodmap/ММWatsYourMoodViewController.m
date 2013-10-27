//
//  ММWatsYourMoodViewController.m
//  moodmap
//
//  Created by Roman Putsykovich on 10/26/13.
//  Copyright (c) 2013 worldmoodmap. All rights reserved.
//

#import "ММWatsYourMoodViewController.h"

@interface __WatsYourMoodViewController ()

@property (strong, nonatomic) NSArray *data;
@property (strong, nonatomic) NSArray *images;
@property (strong, nonatomic) NSIndexPath *checkedIndexPath;

@end

@implementation __WatsYourMoodViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    self.data = @[@"Feeling Awesome",@"Feeling Bad", @"Feeling Loved", @"Feeling Sleepy", @"Feeling Pretty", @"Feeling Determined",@"Feeling Better"];
    self.images = @[@"awesome32.png", @"bad32.png", @"loved32.png", @"sleepy32.png", @"pretty32.png", @"determined32.png",@"better32.png"];
    
    if([[NSUserDefaults standardUserDefaults] objectForKey:MoodImageKey]) {
        NSString *moodImageName = [[NSUserDefaults standardUserDefaults] objectForKey:MoodImageKey];
        NSInteger row = [self.images indexOfObject:moodImageName];
        self.checkedIndexPath = [NSIndexPath indexPathForRow:row inSection:0];
    }
    else {
        self.checkedIndexPath = [NSIndexPath indexPathForRow:0 inSection:0];
    }
}

#pragma mark - Table view data source

- (NSInteger)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (NSInteger)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [self.data count];
}

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath {
        static NSString *CellIdentifier = @"MoodCell";
        UITableViewCell *cell = [tableView dequeueReusableCellWithIdentifier:CellIdentifier forIndexPath:indexPath];
        
        if (!cell) {
            cell = [[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier];
        }
        
        cell.accessoryType = indexPath.row == self.checkedIndexPath.row ? UITableViewCellAccessoryCheckmark : UITableViewCellAccessoryNone;
        cell.textLabel.text = [self.data objectAtIndex:indexPath.row];
        cell.imageView.image = [UIImage imageNamed:[self.images objectAtIndex:indexPath.row]];

        return cell;
}


#pragma mark - Table view delegate

static NSString * const MoodStateKey = @"MoodStateKey";
static NSString * const MoodImageKey = @"MoodImageKey";

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath {
    if (indexPath.row != self.checkedIndexPath.row) {
        UITableViewCell *newCell = [tableView cellForRowAtIndexPath:indexPath];
        newCell.accessoryType = UITableViewCellAccessoryCheckmark;
        
        UITableViewCell *oldCell = [tableView cellForRowAtIndexPath:self.checkedIndexPath];
        oldCell.accessoryType = UITableViewCellAccessoryNone;
        
        self.checkedIndexPath = indexPath;
        
        [[NSUserDefaults standardUserDefaults] setObject:[self.data objectAtIndex:indexPath.row] forKey:MoodStateKey];
        [[NSUserDefaults standardUserDefaults] setObject:[self.images objectAtIndex:indexPath.row] forKey:MoodImageKey];
    }
    [tableView deselectRowAtIndexPath:indexPath animated:YES];
}

@end
